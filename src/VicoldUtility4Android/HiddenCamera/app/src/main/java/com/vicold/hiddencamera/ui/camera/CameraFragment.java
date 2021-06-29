package com.vicold.hiddencamera.ui.camera;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Looper;
import android.util.Size;
import android.view.LayoutInflater;
import android.view.OrientationEventListener;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.camera.core.CameraSelector;
import androidx.camera.core.ImageAnalysis;
import androidx.camera.core.ImageCapture;
import androidx.camera.core.ImageCaptureException;
import androidx.camera.core.ImageProxy;
import androidx.camera.core.Preview;
import androidx.camera.lifecycle.ProcessCameraProvider;
import androidx.camera.view.PreviewView;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.LifecycleOwner;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;

import com.google.common.util.concurrent.ListenableFuture;
import com.vicold.hiddencamera.databinding.FragmentCameraBinding;

import java.io.File;
import java.text.SimpleDateFormat;
import java.util.Locale;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class CameraFragment extends Fragment {

    private static final String[] CAMERA_PERMISSION = new String[]{Manifest.permission.CAMERA,
            Manifest.permission.WRITE_EXTERNAL_STORAGE, Manifest.permission.READ_EXTERNAL_STORAGE};
    private static final int CAMERA_REQUEST_CODE = 10;
    private CameraViewModel cameraViewModel;
    private FragmentCameraBinding binding;

    private PreviewView previewView;
    private ListenableFuture<ProcessCameraProvider> cameraProviderFuture;
    private TextView textView;
    private ImageCapture imageCapture;
    private ExecutorService cameraExecutor;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        if (!hasCameraPermission()) {
            requestPermission();
        }

        cameraViewModel =
                new ViewModelProvider(this).get(CameraViewModel.class);

        binding = FragmentCameraBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        cameraExecutor = Executors.newSingleThreadExecutor();
        previewView = binding.previewView;
        cameraProviderFuture = ProcessCameraProvider.getInstance(getActivity().getApplicationContext());
        cameraProviderFuture.addListener(new Runnable() {
            @Override
            public void run() {
                try {
                    ProcessCameraProvider cameraProvider = cameraProviderFuture.get();
                    bindImageAnalysis(cameraProvider);
                } catch (ExecutionException | InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }, ContextCompat.getMainExecutor(getActivity().getApplicationContext()));


        Button enableCamera = binding.cameraCaptureButton;
        enableCamera.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //take photo
                takephoto();
            }
        });
        return root;
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }

    private boolean hasCameraPermission() {
        return ContextCompat.checkSelfPermission(
                getActivity().getApplicationContext(),
                Manifest.permission.CAMERA
        ) == PackageManager.PERMISSION_GRANTED;
    }

    private void requestPermission() {
        ActivityCompat.requestPermissions(
                getActivity(),
                CAMERA_PERMISSION,
                CAMERA_REQUEST_CODE
        );
    }

    private void enableCamera() {
//        Intent intent = new Intent(this, CameraActivity.class);
//        startActivity(intent);
    }

    private void bindImageAnalysis(@NonNull ProcessCameraProvider cameraProvider) {
        ImageAnalysis imageAnalysis =
                new ImageAnalysis.Builder().setTargetResolution(new Size(1280, 720))
                        .setBackpressureStrategy(ImageAnalysis.STRATEGY_KEEP_ONLY_LATEST).build();
        imageAnalysis.setAnalyzer(ContextCompat.getMainExecutor(getActivity().getApplicationContext()), new ImageAnalysis.Analyzer() {
            @Override
            public void analyze(@NonNull ImageProxy image) {
                image.close();
            }
        });
        OrientationEventListener orientationEventListener = new OrientationEventListener(getActivity().getApplicationContext()) {
            @Override
            public void onOrientationChanged(int orientation) {
                //textView.setText(Integer.toString(orientation));
            }
        };

        //3 拍照 接口
        imageCapture = new ImageCapture.Builder().build();
        orientationEventListener.enable();
        Preview preview = new Preview.Builder().build();
        CameraSelector cameraSelector = new CameraSelector.Builder()
                .requireLensFacing(CameraSelector.LENS_FACING_BACK).build();
        preview.setSurfaceProvider(previewView.getSurfaceProvider());
        cameraProvider.bindToLifecycle((LifecycleOwner) this, cameraSelector, preview, imageCapture, imageAnalysis);
    }

    private void takephoto() {
        //1 设置要保存的路径和文件名字
        File photoFile = new File(getActivity().getApplicationContext().getExternalFilesDir(Environment.DIRECTORY_PICTURES), "image-" + new SimpleDateFormat("yyyyMMddHHmmssSSS", Locale.CHINA).format(System.currentTimeMillis()) + ".jpg");
        //2 定义 拍摄imageCapture实例
        ImageCapture.OutputFileOptions outputFileOptions =
                new ImageCapture.OutputFileOptions.Builder(photoFile).build();
        imageCapture.takePicture(outputFileOptions, cameraExecutor, new ImageCapture.OnImageSavedCallback() {
            @Override
            public void onImageSaved(@NonNull ImageCapture.OutputFileResults outputFileResults) {
                // 先获取路径
                Uri uri= Uri.fromFile(photoFile);
//                if(Build.VERSION.SDK_INT>=Build.VERSION_CODES.M){
//                    // 把任务发送给消息队列
//                    imageView.post(new Runnable() {
//                        @Override
//                        public void run() {
//                            // 用glide加载这个图片
//                            Glide.with(imageView).load(uri).into(imageView);
//                        }
//                    });
//                }
                Looper.prepare();
                Toast.makeText(getActivity().getApplicationContext(), uri.toString(), Toast.LENGTH_SHORT).show();
                Looper.loop();
            }

            @Override
            public void onError(@NonNull ImageCaptureException exception) {
                //"保存失败"+
            }
        });
    }
}